import { createStore, type StoreApi } from "zustand"

type BaseStoreFunctions<Name extends string, T> = {
	[P in `set${Name}`]: (value: T) => void
} & {
	[P in `get${Name}`]: () => T
} & {
	[P in `update${Name}`]: (value: T) => void
}

type BaseStoreState<Name extends string, T> = {
	[P in Name]: T
}

type StoreState<
	Name extends string,
	T,
	Options extends Record<string, unknown> = Record<string, never>,
> = BaseStoreState<Name, T> & BaseStoreFunctions<Name, T> & Options

type GetStore<T> = StoreApi<T>["getState"]
type SetStore<T> = StoreApi<T>["setState"]

type StoreBaseOptions<
	Name extends string,
	T,
	Options extends Record<string, unknown> = Record<string, never>,
> = {
	getItem?: GetStore<StoreState<Name, T, Options>>
	setItem?: SetStore<StoreState<Name, T, Options>>
	updateItem?: SetStore<StoreState<Name, T, Options>>
}

export const storeCreate = <
	T,
	Name extends string,
	Options extends Record<string, unknown> = Record<string, never>,
>(
	itemsName: Name,
	initialValue: T,
	{ getItem, setItem, updateItem }: StoreBaseOptions<Name, T, Options> = {},
	options?: (
		set: SetStore<StoreState<Name, T, Options>>,
		get: GetStore<StoreState<Name, T, Options>>,
	) => Options,
) =>
	createStore<StoreState<Name, T, Options>>((set, get) => {
		const extra = options?.(set, get) ?? ({} as Options)

		return {
			[itemsName]: initialValue,
			[`get${itemsName}`]: getItem
				? () => getItem()
				: () => get()[itemsName],
			[`set${itemsName}`]: (value: T) =>
				(setItem ?? set)({ [itemsName]: value } as Partial<
					StoreState<Name, T, Options>
				>),
			[`update${itemsName}`]: (value: T) =>
				(updateItem ?? set)({ [itemsName]: value } as Partial<
					StoreState<Name, T, Options>
				>),
			...extra,
		} as StoreState<Name, T, Options>
	})
