import { UserAvatarId } from './UserAvatarId';

export interface AccountInfo {
  id: string;
  avatar: keyof typeof UserAvatarId;
  name: string;
  level: number;
  power: number;
  lastUpdated: string;
}
