import { Uuid } from "../uuid"

type BrandUuid = Uuid & {
	readonly __brand: symbol
}

export type BrandedUuid<B extends symbol> = Uuid & {
	readonly __brand: B
}

interface IBaseEntity<Uuid extends BrandUuid = BrandUuid> {
	uuid: Uuid
}

interface IBaseEntityWithVersion<
	Uuid extends BrandUuid = BrandUuid,
> extends IBaseEntity<Uuid> {
	version: number
}

export type { IBaseEntity, IBaseEntityWithVersion }
