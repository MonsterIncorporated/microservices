export enum TransactionStatus{
    "PENDING" = "PENDING",
    "COMPLETED" = "COMPLETED"
}

export type CreateTransactionDto = {
  userId: string;
  requiredTokens: number;
  numberOfDigits: number;
  status: TransactionStatus;
};
