import { storeCreate } from "@/shared/lib/storeCreate"
import { TGroup } from "./TGroup"

export const useGroupStore = storeCreate<TGroup[], "groups">("groups", [])
