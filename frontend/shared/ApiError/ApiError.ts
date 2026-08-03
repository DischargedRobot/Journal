export class ApiError extends Error {
	static readonly errorType = "APIError" // лень писать постоянно при создании
	constructor(
		public httpCode: number | null,
		public statusCode: string,
		public title: string,
		public message: string,
		public detail?: string | null, // поле для FFAPIError
	) {
		super(message || `Error ${ApiError.errorType}`)

		this.name = "ApiError"
	}
}

export function isApiError(error: unknown): error is ApiError {
	return (
		error instanceof ApiError ||
		(typeof error === "object" &&
			error !== null &&
			//
			"statusCode" in error &&
			typeof error.statusCode === "string" &&
			/d.d.d/.test(error.statusCode) &&
			//
			"title" in error &&
			typeof error.title === "string" &&
			//
			"message" in error &&
			typeof error.message === "string")
	)
}

export const ApiErrors = {
	NETWORK: new ApiError(null, "NETWORK", "Network error", "Сетевая ошибка"),
	BAD_REQUEST: new ApiError(
		400,
		"BAD_REQUEST",
		"Bad request",
		"Неверный запрос",
	),
	FORBIDEN: new ApiError(403, "FORBIDEN", "Forbidden", "Доступ запрещён"),
	UNAUTHORIZED: new ApiError(
		401,
		"UNAUTHORIZED",
		"Unauthorized",
		"Неавторизован",
	),
	NOT_FOUND: new ApiError(404, "NOT_FOUND", "Not found", "Ресурс не найден"),
	SERVER: new ApiError(500, "SERVER_ERROR", "Server error", "Ошибка сервера"),
} as const

export const mapApiErrors = (
	status: number | null | undefined,
	message?: string,
): ApiError => {
	let error: ApiError

	switch (status) {
		case null:
			error = ApiErrors.NETWORK
			break
		case 400:
			error = ApiErrors.BAD_REQUEST
			break
		case 403:
			error = ApiErrors.FORBIDEN
			break
		case 401:
			error = ApiErrors.UNAUTHORIZED
			break
		case 404:
			error = ApiErrors.NOT_FOUND
			break
		case 500:
			error = ApiErrors.SERVER
			break
		default:
			error = new ApiError(
				status ? status : 0,
				"1.0.0",
				"UNKNOW",
				"Неизвестная ошибка",
			)
	}

	if (message) {
		error.message = message
	}

	return error
}
