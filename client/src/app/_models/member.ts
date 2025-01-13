import { Photo } from "./photo"

export interface Member {
    id: number
    userName: string
    photoUrl: any
    age: number
    knownAs: string
    created: Date
    lastActive: Date
    gender: string
    introduction: string
    lookingFor: string
    intrests: any
    city: string
    country: string
    photos: Photo[]
  }
  
