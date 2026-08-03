import { Uuid } from "../uuid"

interface IBaseEntity {
	uuid: Uuid
}

interface IBaseEntityWithVersion extends IBaseEntity {
	version: number
}

export type { IBaseEntity, IBaseEntityWithVersion }
