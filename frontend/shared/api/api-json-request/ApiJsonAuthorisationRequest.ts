import { ApiJsonRequest } from "."
import { isApiError, mapApiErrors } from "../api-error"
import { AuthApi } from "../auth"

let refreshPromise: Promise<void> | null = null

const refreshSessionOnce = () => {
	if (!refreshPromise) {
		refreshPromise = AuthApi.refresh().finally(() => {
			refreshPromise = null
		})
	}
	return refreshPromise
}

export const ApiJsonAuthorisationRequest = async (
	endpoint: string,
	options?: RequestInit,
) => {
	try {
		return await ApiJsonRequest(endpoint)
	} catch (error) {
		if (!isApiError(error)) {
			throw mapApiErrors(0)
		}

		if (error.httpCode === 401) {
			try {
				await refreshSessionOnce()
			} catch (error) {
				if (!isApiError(error)) {
					throw mapApiErrors(0)
				}
				// если ошибка 401 снова появилась, значит рефреш не прошёл и выходим из системы,
				// иначе просто уходим на 500, и оставляем пользователю учётку
				if (error.httpCode === 401) {
					await AuthApi.logOut()
					window.location.href = "/login"
				} else {
					window.location.href = "/500"
				}
			}
			return await ApiJsonRequest(endpoint, options)
		}

		throw error
	}
}
