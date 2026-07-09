import { NavigateOptions } from "next/dist/shared/lib/app-router-context.shared-runtime"
import { ApiError, isApiError, mapApiErrors } from "./ApiError"

interface CustomErrorHandler {
	error: ApiError
	handler: (error: ApiError) => void
}

/**
 * Фабрика обработчика ошибок API. Возвращает функцию handleError(error).
 *
 * @param customHandlers - список кастомных обработчиков по статус-коду
 * @param defaultHandler - опциональный общий обработчик
 * @param navigate - опциональная функция навигации (path, options?) => void, options: NavigateOptions
 */
export const createApiErrorHandler = (
	customHandlers: CustomErrorHandler[] = [],
	navigate?: (path: string, options?: NavigateOptions) => void,
	defaultHandler?: (error: ApiError) => void,
) => {
	const handleError = (error: ApiError | unknown) => {
		if (isApiError(error)) {
			const customHandler = customHandlers.find((handler) => {
				return handler.error.statusCode === error.statusCode
			})

			if (customHandler) {
				customHandler.handler(error)
				return
			}
		}

		const apiError: ApiError = isApiError(error)
			? error
			: mapApiErrors(null)

		if (defaultHandler !== undefined) {
			defaultHandler(apiError)
			return
		}

		switch (apiError.httpCode) {
			case 401:
				if (navigate) {
					navigate("/auth")
				} else if (typeof window !== "undefined") {
					window.location.href = "/auth"
				}
				break
			case 403:
				if (navigate) {
					navigate("/auth")
				} else if (typeof window !== "undefined") {
					window.location.href = "/auth"
				}
				break
			case 404:
				break
			case 409:
				break
			case 500:
				break
			default:
				break
		}
	}

	return handleError
}
