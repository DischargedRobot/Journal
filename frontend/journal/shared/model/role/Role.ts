import { TPerm } from "./Perm"
import { Uuid } from "../utility-types/uuid"

export type TRole = {
	uuid: Uuid
	name: string
	permissions: TPerm[]
}
