<template>
  <div class="page-container">
    <el-card shadow="hover">
      <template #header>
        <div class="card-header">
          <span>物料分类</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon> 新建分类
          </el-button>
        </div>
      </template>

      <el-row :gutter="16">
        <el-col :span="8">
          <el-tree
            :data="treeData"
            :props="{ label: 'classificationName', children: 'children' }"
            node-key="id"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick"
          />
        </el-col>
        <el-col :span="16">
          <WmsTable
            :data="displayTableData"
            :loading="loading"
            :total="total"
            v-model:current-page="pagination.currentPage"
            v-model:page-size="pagination.pageSize"
            :page-sizes="pagination.pageSizes"
            @page-change="handlePageChange"
            @size-change="handleSizeChange"
          >
            <el-table-column prop="classificationCode" label="分类编码" />
            <el-table-column prop="classificationName" label="分类名称" />
            <el-table-column prop="classificationLevel" label="层级" />
            <el-table-column prop="parentClassificationName" label="上级分类">
              <template #default="{ row }">
                {{ row.parentClassificationName || '-' }}
              </template>
            </el-table-column>
            <el-table-column label="操作" width="180" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleEdit(row as MaterialClassificationDto)">编辑</el-button>
                <el-button link type="danger" @click="handleDelete(row as MaterialClassificationDto)">删除</el-button>
              </template>
            </el-table-column>
          </WmsTable>
        </el-col>
      </el-row>
    </el-card>

    <WmsDialog
      :title="formData.id ? '编辑分类' : '新建分类'"
      :visible="visible"
      :confirm-loading="submitting"
      show-footer
      width="500px"
      @close="closeForm"
      @cancel="closeForm"
      @confirm="handleSubmit"
    >
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-form-item label="分类编码" prop="classificationCode">
          <el-input v-model="formData.classificationCode" placeholder="请输入分类编码" />
        </el-form-item>
        <el-form-item label="分类名称" prop="classificationName">
          <el-input v-model="formData.classificationName" placeholder="请输入分类名称" />
        </el-form-item>
        <el-form-item label="上级分类">
          <el-select v-model="formData.parentClassificationId" placeholder="请选择上级分类" clearable style="width: 100%">
            <el-option
              v-for="item in allClassifications"
              :key="item.id"
              :label="item.classificationName"
              :value="item.id"
              :disabled="item.id === formData.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
    </WmsDialog>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue';
import { Plus } from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import WmsTable from '@/components/common/WmsTable.vue';
import WmsDialog from '@/components/common/WmsDialog.vue';
import { useTable } from '@/hooks/useTable';
import { useForm } from '@/hooks/useForm';
import {
  getClassifications,
  createClassification,
  updateClassification,
  deleteClassification,
} from '@/api/material';
import type { MaterialClassificationDto } from '@/api/material';

interface ClassificationTreeNode extends MaterialClassificationDto {
  children?: ClassificationTreeNode[];
  parentName?: string;
}

const { loading, tableData, total, pagination, handlePageChange, handleSizeChange, handleSearch } =
  useTable<MaterialClassificationDto>('/api/v1/material/classifications');

const { formRef, formData, formRules, submitting, visible, openForm, closeForm, submitForm } =
  useForm<Partial<MaterialClassificationDto>>({
    classificationCode: '',
    classificationName: '',
    parentClassificationId: '',
    classificationLevel: 1,
  });

formRules.value = {
  classificationCode: [{ required: true, message: '请输入分类编码', trigger: 'blur' }],
  classificationName: [{ required: true, message: '请输入分类名称', trigger: 'blur' }],
};

const allClassifications = ref<MaterialClassificationDto[]>([]);
const selectedNode = ref<ClassificationTreeNode | null>(null);

const displayTableData = computed(() => {
  const map = new Map(allClassifications.value.map((c) => [c.id, c.classificationName]));
  return tableData.value.map((row) => ({
    ...row,
    parentClassificationName: row.parentClassificationId ? map.get(row.parentClassificationId) || '-' : '-',
  }));
});

const treeData = computed<ClassificationTreeNode[]>(() => {
  const map = new Map<string, ClassificationTreeNode>();
  const roots: ClassificationTreeNode[] = [];
  allClassifications.value.forEach((item) => {
    map.set(item.id, { ...item, children: [] });
  });
  allClassifications.value.forEach((item) => {
    const node = map.get(item.id)!;
    if (item.parentClassificationId && map.has(item.parentClassificationId)) {
      map.get(item.parentClassificationId)!.children!.push(node);
    } else {
      roots.push(node);
    }
  });
  return roots;
});

async function loadAllClassifications() {
  try {
    const res = await getClassifications({ maxResultCount: 1000 });
    allClassifications.value = res.items;
  } catch {
    ElMessage.error('加载分类失败');
  }
}

function handleCreate() {
  openForm({
    classificationCode: '',
    classificationName: '',
    parentClassificationId: selectedNode.value?.id || '',
    classificationLevel: (selectedNode.value?.classificationLevel || 0) + 1,
  });
}

function handleEdit(row: MaterialClassificationDto) {
  openForm({ ...row });
}

function handleNodeClick(data: ClassificationTreeNode) {
  selectedNode.value = data;
}

async function handleDelete(row: MaterialClassificationDto) {
  try {
    await ElMessageBox.confirm('确认删除该分类？', '提示', { type: 'warning' });
    await deleteClassification(row.id);
    ElMessage.success('删除成功');
    handleSearch();
    loadAllClassifications();
  } catch {
    // Cancel
  }
}

async function handleSubmit() {
  if (formData.id) {
    const updateData = {
      classificationName: formData.classificationName || '',
      parentClassificationId: formData.parentClassificationId,
      classificationLevel: formData.classificationLevel ?? 1,
    };
    const success = await submitForm(async () => {
      await updateClassification(formData.id!, updateData);
    }, '更新成功');
    if (success) {
      handleSearch();
      loadAllClassifications();
    }
  } else {
    const createData = {
      classificationCode: formData.classificationCode || '',
      classificationName: formData.classificationName || '',
      parentClassificationId: formData.parentClassificationId,
      classificationLevel: formData.classificationLevel ?? 1,
    };
    const success = await submitForm(async () => {
      await createClassification(createData);
    }, '创建成功');
    if (success) {
      handleSearch();
      loadAllClassifications();
    }
  }
}

onMounted(() => {
  loadAllClassifications();
});

handleSearch();
</script>

<style scoped lang="scss">
.page-container {
  padding: 0;
}
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
