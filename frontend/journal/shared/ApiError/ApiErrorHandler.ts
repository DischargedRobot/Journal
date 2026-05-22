import { useRouter } from "next/navigation"
import { ApiError, isApiError, mapApiErrors } from "./ApiError"
import { useCallback } from "react"

interface CustomErrorHandler {
	error: ApiError
	handler: (error: ApiError) => void
}

export const useApiErrorHandler = (
	customHandlers: CustomErrorHandler[] = [],
	defaultHandler?: (error: ApiError) => void,
) => {
	const router = useRouter()

	// чтобы при рендере компонента не перезаписывалась повторно
	const handleError = useCallback(
		(error: ApiError | unknown) => {
			// Сначала проверяем кастомные обработчики

			if (isApiError(error)) {
				// console.log(
				// 	"isFFApiError handleError",
				// 	Object.values(error),
				// 	isFFApiError(error),
				// )

				const customHandler = customHandlers.find((handler) => {
					return handler.error.statusCode === error.statusCode
				})

				if (customHandler) {
					customHandler.handler(error)
					return
				}
			}

			const ApiError: ApiError = isApiError(error)
				? error
				: mapApiErrors(null)

			if (defaultHandler !== undefined) {
				defaultHandler(ApiError)
				return
			}
			// Если кастомного обработчика нет, используем что есть
			switch (ApiError.httpCode) {
				case 401:
					router.push("/auth")
					break
				case 403:
					router.push("/auth")
					break
				case 404:
					// router.push("/notFound")
					break
				case 409:
					// router.push("/notFound")
					break
				case 500:
					// router.push("/internal")
					break
				default:
					if (error instanceof Error) {
					} else {
					}
			}
		},
		[router, customHandlers, defaultHandler],
	)

	return handleError
}
