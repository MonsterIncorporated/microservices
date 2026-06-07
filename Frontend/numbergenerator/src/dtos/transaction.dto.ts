export enum TransactionStatus{
    "PENDING",
    "COMPLETED"
}

export type TransactionDto = {
  transactionId: string;
  userId: string;
  requiredTokens: number;
  numberOfDigits: number;
  status: TransactionStatus;
};
