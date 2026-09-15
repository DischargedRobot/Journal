type TNonNegative<T extends number | string> = `${T}` extends `-${string}`
	? never
	: T

type TStatusCode<S extends string> =
	S extends `${infer A}.${infer B}.${infer C}`
		? TNonNegative<`${A}`> extends never
			? never
			: TNonNegative<`${B}`> extends never
				? never
				: TNonNegative<`${C}`> extends never
					? never
					: S
		: never

export class ApiError<S extends string = string> extends Error {
	static readonly errorType = "APIError" // лень писать постоянно при создании
	constructor(
		public httpCode: number | null,
		public statusCode: TStatusCode<S>,
		public title: string,
		public message: string,
		public field?: string,
		public details?: string | null, // поле для APIError
	) {
		super(message || `Error ${ApiError.errorType}`)

		this.name = "ApiError"
	}
}

const STATUS_CODE_REGEX = /^\d+\.\d+\.\d+$/

export function isApiError(error: unknown): error is ApiError {
	// console.log(
	// 	"error",
	// 	error,
	// 	error instanceof ApiError ||
	// 		(typeof error === "object" &&
	// 			error !== null &&
	// 			//
	// 			"statusCode" in error &&
	// 			typeof error.statusCode === "string" &&
	// 			STATUS_CODE_REGEX.test(error.statusCode) &&
	// 			//
	// 			"title" in error &&
	// 			typeof error.title === "string" &&
	// 			//
	// 			"message" in error &&
	// 			typeof error.message === "string"),
	// )

	return (
		error instanceof ApiError ||
		(typeof error === "object" &&
			error !== null &&
			//
			"statusCode" in error &&
			typeof error.statusCode === "string" &&
			STATUS_CODE_REGEX.test(error.statusCode) &&
			//
			"title" in error &&
			typeof error.title === "string" &&
			//
			"message" in error &&
			typeof error.message === "string")
	)
}

export const ApiErrors = {
	NETWORK: new ApiError(null, "1.0.0", "Network error", "Сетевая ошибка"),
	BAD_REQUEST: new ApiError(400, "1.0.0", "Bad request", "Неверный запрос"),
	FORBIDEN: new ApiError(403, "1.0.0", "Forbidden", "Доступ запрещён"),
	UNAUTHORIZED: new ApiError(401, "1.0.0", "Unauthorized", "Неавторизован"),
	NOT_FOUND: new ApiError(404, "1.0.0", "Not found", "Ресурс не найден"),
	CONFLICT: new ApiError(
		409,
		"1.0.0",
		"Conflict",
		"Ресурс с таким парамметром уже существует",
	),
	SERVER: new ApiError(500, "1.0.0", "Server error", "Ошибка сервера"),
} as const

export const mapApiErrors = (
	httpCode: number | null | undefined,
	message?: string,
): ApiError => {
	let error: ApiError

	const defaultError = new ApiError(
		httpCode ? httpCode : 0,
		"1.0.0",
		"UNKNOW",
		"Неизвестная ошибка",
	)
	error =
		Object.values(ApiErrors).find((e) =>
			httpCode ? e.httpCode === httpCode : false,
		) ?? defaultError

	if (message) {
		error.message = message
	}

	return error
}
