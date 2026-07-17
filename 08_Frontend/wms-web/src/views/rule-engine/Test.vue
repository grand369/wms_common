<template>
  <div class="page-container">
    <el-page-header title="返回规则列表" content="规则测试" @back="goBack" />

    <el-card shadow="hover" class="form-card">
      <div class="test-form">
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item label="选择规则" label-width="80px">
              <el-select
                v-model="selectedRuleId"
                placeholder="请选择要测试的规则"
                filterable
                clearable
                style="width: 100%"
                @change="handleRuleChange"
              >
                <el-option
                  v-for="rule in rules"
                  :key="rule.id"
                  :label="`${rule.name} (${rule.code})`"
                  :value="rule.id"
                >
                  <div class="rule-option">
                    <span>{{ rule.name }}</span>
                    <el-tag size="small" type="info" class="rule-type-tag">
                      {{ rule.ruleType }}
                    </el-tag>
                  </div>
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="16" v-if="selectedRule">
          <el-col :span="24">
            <el-form-item label="规则类型" label-width="80px">
              <el-tag>{{ selectedRule.ruleType }}</el-tag>
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="16" v-if="selectedRule">
          <el-col :span="24">
            <el-form-item label="输入 JSON" label-width="80px">
              <el-input
                v-model="inputJson"
                type="textarea"
                :rows="8"
                placeholder='{"key": "value", ...}'
                class="json-input"
              />
            </el-form-item>
          </el-col>
        </el-row>

        <el-row :gutter="16" v-if="selectedRule">
          <el-col :span="24">
            <div class="test-action">
              <el-button type="primary" :loading="executing" @click="handleExecute">
                <el-icon><CaretRight /></el-icon> 执行测试
              </el-button>
              <el-button @click="handleReset">重置</el-button>
            </div>
          </el-col>
        </el-row>
      </div>
    </el-card>

    <!-- Result Section -->
    <el-card shadow="hover" class="result-card" v-if="executionResult">
      <template #header>
        <div class="card-header">
          <span>执行结果</span>
          <el-tag :type="executionResult.success ? 'success' : 'danger'" effect="dark">
            {{ executionResult.success ? '成功' : '失败' }}
          </el-tag>
        </div>
      </template>

      <el-result
        :icon="executionResult.success ? 'success' : 'error'"
        :title="executionResult.success ? '规则执行成功' : '规则执行失败'"
        size="small"
      >
        <template #sub-title>
          <div class="result-detail">
            <el-descriptions :column="1" border size="small">
              <el-descriptions-item label="执行状态">
                <el-tag :type="executionResult.success ? 'success' : 'danger'" size="small">
                  {{ executionResult.success ? 'PASS' : 'FAIL' }}
                </el-tag>
              </el-descriptions-item>
              <el-descriptions-item label="输出结果">
                <pre class="result-output">{{ formatOutput(executionResult.output) }}</pre>
              </el-descriptions-item>
            </el-descriptions>

            <div class="messages-section" v-if="executionResult.messages && executionResult.messages.length > 0">
              <el-divider content-position="left">执行消息</el-divider>
              <div class="message-list">
                <div
                  v-for="(msg, index) in executionResult.messages"
                  :key="index"
                  class="message-item"
                >
                  <el-tag
                    :type="executionResult.success ? 'success' : 'danger'"
                    size="small"
                    effect="plain"
                  >
                    {{ index + 1 }}
                  </el-tag>
                  <span class="message-text">{{ msg }}</span>
                </div>
              </div>
            </div>
          </div>
        </template>
      </el-result>
    </el-card>

    <el-empty v-if="!selectedRule" description="请选择一个规则进行测试" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElMessage } from 'element-plus';
import { CaretRight } from '@element-plus/icons-vue';
import {
  getBusinessRules,
  executeRule,
  type BusinessRuleDto,
  type RuleExecutionResult,
} from '@/api/ruleEngine';

const router = useRouter();

const rules = ref<BusinessRuleDto[]>([]);
const selectedRuleId = ref('');
const selectedRule = ref<BusinessRuleDto | null>(null);
const inputJson = ref('');
const executing = ref(false);
const executionResult = ref<RuleExecutionResult | null>(null);

function formatOutput(output: any): string {
  if (output === null || output === undefined) return 'null';
  if (typeof output === 'object') return JSON.stringify(output, null, 2);
  return String(output);
}

async function loadRules() {
  try {
    const result = await getBusinessRules({ maxResultCount: 100 });
    rules.value = result.items || [];
  } catch {
    ElMessage.error('加载规则列表失败');
  }
}

function handleRuleChange() {
  if (!selectedRuleId.value) {
    selectedRule.value = null;
    executionResult.value = null;
    return;
  }
  const found = rules.value.find((r) => r.id === selectedRuleId.value);
  selectedRule.value = found || null;
  executionResult.value = null;
}

async function handleExecute() {
  if (!selectedRule.value) {
    ElMessage.warning('请先选择规则');
    return;
  }

  let parsedInput: any;
  try {
    parsedInput = inputJson.value.trim()
      ? JSON.parse(inputJson.value)
      : {};
  } catch {
    ElMessage.error('JSON 格式错误，请检查输入');
    return;
  }

  executing.value = true;
  executionResult.value = null;
  try {
    const result = await executeRule({
      ruleType: selectedRule.value.ruleType,
      input: parsedInput,
    });
    executionResult.value = result;
    if (result.success) {
      ElMessage.success('规则执行成功');
    } else {
      ElMessage.warning('规则执行失败，请查看消息');
    }
  } catch {
    ElMessage.error('规则执行出错');
  } finally {
    executing.value = false;
  }
}

function handleReset() {
  selectedRuleId.value = '';
  selectedRule.value = null;
  inputJson.value = '';
  executionResult.value = null;
}

function goBack() {
  router.push('/rule-engine/rules');
}

onMounted(() => {
  loadRules();
});
</script>

<style scoped lang="scss">
@use '@/styles/variables.scss' as *;

.page-container {
  padding: 0;
}

.form-card {
  margin-top: 16px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.test-form {
  padding: 8px 0;
}

.rule-option {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;

  .rule-type-tag {
    margin-left: 8px;
  }
}

.json-input {
  font-family: 'Courier New', monospace;
  font-size: 13px;
}

.test-action {
  text-align: right;
  padding: 8px 0;
}

.result-card {
  margin-top: 16px;
}

.result-detail {
  text-align: left;
  width: 100%;
}

.result-output {
  margin: 0;
  padding: 8px;
  background: $wms-bg-base;
  border-radius: $wms-radius-sm;
  font-size: 13px;
  font-family: 'Courier New', monospace;
  white-space: pre-wrap;
  word-break: break-all;
  max-height: 200px;
  overflow-y: auto;
}

.messages-section {
  margin-top: 16px;
}

.message-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.message-item {
  display: flex;
  align-items: flex-start;
  gap: 8px;

  .message-text {
    font-size: $wms-font-size-body;
    color: $wms-text-regular;
    line-height: 1.5;
  }
}
</style>
