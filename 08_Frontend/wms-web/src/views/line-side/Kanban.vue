<template>
  <div class="page-container">
    <el-card shadow="hover" class="header-card">
      <template #header>
        <div class="card-header">
          <span>
            <el-icon><DataBoard /></el-icon>
            {{ kanbanData?.stationName || '工位看板' }}
            <el-tag v-if="kanbanData?.lastUpdated" type="info" size="small" effect="plain" class="update-tag">
              最后更新：{{ kanbanData.lastUpdated }}
            </el-tag>
          </span>
          <div class="header-actions">
            <WmsSignalRIndicator show-label />
            <el-select
              v-model="selectedStationId"
              placeholder="切换工位"
              clearable
              style="width: 220px"
              @change="handleStationChange"
            >
              <el-option
                v-for="opt in stationOptions"
                :key="opt.value"
                :label="opt.label"
                :value="opt.value"
              />
            </el-select>
            <el-button :icon="Refresh" @click="loadKanban">刷新</el-button>
          </div>
        </div>
      </template>
    </el-card>

    <el-card shadow="hover" class="board-card">
      <div class="kanban-grid">
        <KanbanCard
          v-for="m in kanbanData?.materials || []"
          :key="m.materialId"
          :material="m"
          @replenish="handleReplenish"
        />
        <el-empty
          v-if="!kanbanData || (kanbanData.materials || []).length === 0"
          description="暂无看板数据"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue';
import { useRoute } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { DataBoard, Refresh } from '@element-plus/icons-vue';
import WmsSignalRIndicator from '@/components/common/WmsSignalRIndicator.vue';
import KanbanCard from '@/components/common/KanbanCard.vue';
import { getKanbanData, getLineSideStations, triggerReplenishment } from '@/api/lineSide';
import type { KanbanDataDto, KanbanMaterialDto, LineSideStationDto } from '@/api/lineSide';

const route = useRoute();
const kanbanData = ref<KanbanDataDto | null>(null);
const stationOptions = ref<Array<{ value: string; label: string }>>([]);

const selectedStationId = ref<string | undefined>(route.params.id as string | undefined);

function handleStationChange(value: string) {
  selectedStationId.value = value;
  loadKanban();
}

async function loadStations() {
  try {
    const res = await getLineSideStations({ skipCount: 0, maxResultCount: 1000 });
    stationOptions.value = (res.items || []).map((s: LineSideStationDto) => ({
      value: s.id,
      label: `${s.code} - ${s.name}`,
    }));
  } catch {
    stationOptions.value = [];
  }
}

async function loadKanban() {
  try {
    kanbanData.value = await getKanbanData(selectedStationId.value);
  } catch {
    ElMessage.error('加载看板数据失败');
  }
}

async function handleReplenish(material: KanbanMaterialDto) {
  try {
    const stationId = kanbanData.value?.stationId;
    if (!stationId) {
      ElMessage.warning('未选择工位，无法触发补料');
      return;
    }
    const shortage = Math.max(material.requiredQty - material.currentQty, 1);
    const { value: qty } = await ElMessageBox.prompt(
      `物料 ${material.materialName} 当前 ${material.currentQty}/${material.requiredQty}，请输入补料数量`,
      '触发补料',
      {
        inputValue: String(shortage),
        inputValidator: (val: string) => {
          const num = Number(val);
          if (!val || Number.isNaN(num) || num <= 0) {
            return '请输入大于0的数量';
          }
          return true;
        },
        inputType: 'number',
      }
    ).catch(() => ({ value: '' }));
    if (!qty) return;
    await triggerReplenishment({
      stationId,
      materialId: material.materialId,
      qty: Number(qty),
    });
    ElMessage.success('补料任务已创建');
    loadKanban();
  } catch (e: any) {
    if (e !== 'cancel') {
      ElMessage.error('补料触发失败');
    }
  }
}

let pollTimer: number | null = null;
function startPolling() {
  pollTimer = window.setInterval(() => {
    loadKanban();
  }, 30000);
}
function stopPolling() {
  if (pollTimer) {
    clearInterval(pollTimer);
    pollTimer = null;
  }
}

watch(() => route.params.id, (val) => {
  if (val) {
    selectedStationId.value = val as string;
    loadKanban();
  }
});

onMounted(async () => {
  await loadStations();
  await loadKanban();
  startPolling();
});
onUnmounted(() => {
  stopPolling();
});
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
}
.header-actions {
  display: flex;
  gap: 12px;
  align-items: center;
}
.update-tag {
  margin-left: 12px;
}
.board-card {
  background: #f5f7fa;
}
.kanban-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 16px;
}
</style>
