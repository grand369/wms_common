import { defineStore } from 'pinia';
import { ref } from 'vue';
import type { TransferDto } from '@/api/transfer';

export const useTransferStore = defineStore('transfer', () => {
  const transfers = ref<Map<string, TransferDto>>(new Map());

  function setTransfer(data: TransferDto) {
    transfers.value.set(data.id, { ...data });
  }

  function getTransfer(id: string): TransferDto | undefined {
    return transfers.value.get(id);
  }

  function updateStatus(id: string, status: number) {
    const existing = transfers.value.get(id);
    if (existing) {
      existing.status = status;
      transfers.value.set(id, { ...existing });
    }
  }

  function removeTransfer(id: string) {
    transfers.value.delete(id);
  }

  function clearAll() {
    transfers.value.clear();
  }

  return { transfers, setTransfer, getTransfer, updateStatus, removeTransfer, clearAll };
});
