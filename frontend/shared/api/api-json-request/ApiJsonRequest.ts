import { isApiError, mapApiErrors } from "../api-error"
import { isConnectionRefused } from "../api-error/ApiError"

const ApiJsonRequest = async <T>(
	endpoint: string,
	options?: RequestInit,
): Promise<T> => {
	try {
		const response = await fetch(endpoint, {
			// credentials: 'include',
			...options,
			headers: {
				"Content-Type": "application/json",
				Accept: "application/json",
				...options?.headers,
			},
			credentials: "include",
		})
		if (!response.ok) {
			const error = await response.json()
			// console.log(error, "error convert", {
			// 	...mapApiErrors(response.status),
			// 	...error,
			// })

			const apiError = {
				...mapApiErrors(response.status),
				...error,
			}
			// console.log(Object.values(apiError), "convert", error)
			throw apiError
		}

		const contentType = response.headers.get("content-type")
		const contenLength = response.headers.get("content-length")

		if (
			contenLength === "0" ||
			// contenLength === null ||
			!contentType?.includes("application/json")
		) {
			return {} as T
		}

		const data: T = await response.json()

		return data
	} catch (error) {
		// если нет сети
		if (error instanceof TypeError && error.message === "Failed to fetch") {
			throw mapApiErrors(1)
		}
		// если нет соединения с сервером
		if (isConnectionRefused(error)) {
			console.log(error, "error is ConnectionRefused")
			throw mapApiErrors(503)
		}
		// если не ApiError, то возвращаем ошибку 0 (хотя такого быть не должно)
		if (!isApiError(error)) {
			console.log(error, "error is not ApiError")
			throw mapApiErrors(0)
		}

		throw error
	}
}

export default ApiJsonRequest
