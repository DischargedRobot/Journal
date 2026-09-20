import { Uuid } from "@/shared/model/utility-types/uuid"

type EntityObject<T> = T extends object
	? T extends Uuid
		? never
		: T extends readonly unknown[]
			? never
			: T
	: never

export type TWithoutEnrich<T> = {
	[K in keyof T as T[K] extends readonly (infer E)[]
		? E extends EntityObject<E>
			? `${Extract<K, string>}Uuids`
			: K
		: T[K] extends EntityObject<T[K]>
			? `${Extract<K, string>}Uuid`
			: K]: T[K] extends readonly (infer E)[]
		? E extends EntityObject<E>
			? E extends { uuis: infer BrandUuid }
				? BrandUuid[]
				: Uuid[]
			: T[K]
		: T[K] extends EntityObject<T[K]>
			? T[K] extends { uuis: infer BrandUuid }
				? BrandUuid
				: Uuid
			: T[K]
}

type CutUuidPrefix<K extends string> = K extends `${infer Prefix}Uuids`
	? Prefix
	: K extends `${infer Prefix}Uuid`
		? Prefix
		: K

export type TToEnrich<WE, Enriched> = {
	[K in keyof WE as CutUuidPrefix<Extract<K, string>>]: CutUuidPrefix<
		Extract<K, string>
	> extends keyof Enriched
		? Enriched[CutUuidPrefix<Extract<K, string>>]
		: WE[K]
}
