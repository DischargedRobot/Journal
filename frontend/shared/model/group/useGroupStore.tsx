import { storeCreate } from "@/shared/lib/storeCreate"
import { TGroup } from "./TGroup"
import type { AnchorHTMLAttributes, ButtonHTMLAttributes } from "react"

export const useGroupStore = storeCreate<TGroup[], "groups">("groups", [])
