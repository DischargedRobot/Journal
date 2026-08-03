export type TPagedResponse<T> = {
	total: number
	offset: number
	size: number
	items: T[]
}
