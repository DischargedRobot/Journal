export class ApiError extends Error {
	static readonly errorType = "APIError" // лень писать постоянно при создании
	constructor(
		public statusCode: number | null,
		public title: string,
		message?: string,
		public detail?: string, // поле для FFAPIError
	) {
		super((message ?? title) || `Error ${ApiError.errorType}`)

		this.name = "ApiError"
	}
}
