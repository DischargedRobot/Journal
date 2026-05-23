import ApiJsonRequest from "../ApiError/ApiJsonRequest"

export type TUsersCreateDto = {
	login: string
	password: string
	email?: string | null
	firstName: string
	lastName: string
	patronymic?: string | null
	rolesUuid: string[]
}

const AUTH_URL = process.env.NEXT_PUBLIC_API_AUTH_URL_V1

const AuthApi = {
	login: async (login: string, password: string) => {
		ApiJsonRequest(`${AUTH_URL}/login`, {
			method: "POST",
			body: JSON.stringify({ login, password }),
		})
	},

	logout: async () => {
		ApiJsonRequest(`${AUTH_URL}/logout`, {
			method: "POST",
		})
	},

	register: async (data: TUsersCreateDto) => {
		return ApiJsonRequest(`${AUTH_URL}/register`, {
			method: "POST",
			body: JSON.stringify(data),
		})
	},

	refresh: async () => {
		return ApiJsonRequest(`${AUTH_URL}/refresh`, {
			method: "POST",
		})
	},
}
export default AuthApi
