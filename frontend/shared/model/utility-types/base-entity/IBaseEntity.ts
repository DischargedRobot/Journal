import { Uuid } from "../uuid"

type Brand = {
	readonly brand: symbol
}

type UniqueBrand = {
	readonly brand: unique symbol
}

interface IBaseEntity<T extends Brand = UniqueBrand> {
	uuid: Uuid & { readonly brand: T["brand"] }
}

interface IBaseEntityWithVersion<
	T extends Brand = UniqueBrand,
> extends IBaseEntity<T> {
	version: number
}

export type { IBaseEntity, IBaseEntityWithVersion }
