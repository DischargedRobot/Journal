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
				return handler.error.httpCode === error.httpCode
			})

			if (customHandler) {
				customHandler.handler(error)
				return
			}
		}

		const apiError: ApiError = isApiError(error) ? error : mapApiErrors(0)

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
			case 409:
			case 500:
			default:
				break
		}
	}

	return handleError
}
