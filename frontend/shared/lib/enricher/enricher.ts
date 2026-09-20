type EnricherData<T> = {
	[K in keyof T]: {
		value: unknown
	}
}[keyof T]

export const enricher = <
	T extends object,
	R extends { [K in keyof T]: unknown },
>(
	initial: T,
	data: R,
): Omit<T, keyof R> & R => {
	return Object.assign(initial, data)
}
