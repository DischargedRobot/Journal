import { storeCreate } from "@/shared/lib/storeCreate"
import { TUser } from "./TUser"

export const useUserStore = storeCreate<TUser[], "users">("users", [])
