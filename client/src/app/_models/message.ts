export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderPhotoUrl: string
    recipientId: number
    reipientUsername: any
    recipientPhotoUrl: string
    content: string
    dateRead?: Date
    messageSent: Date
  }
  