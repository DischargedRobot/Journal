import { ApiJsonRequest } from "../api-json-request"

type TUsersCreateDto = {
	login: string
	password: string
	email?: string | null
	firstName: string
	lastName: string
	patronymic?: string | null
	rolesUuid: string[]
}

const AUTH_URL = process.env.NEXT_PUBLIC_API_AUTH_URL_V1 + "/Auth"
// TODO: брать из userApi
type TUserResponse = {
	uuid: string
	login: string
	email?: string | null
	firstName?: string | null
	lastName?: string | null
	patronymic?: string | null
	tokenVersion: number
	rolesUuid?: string[] | null
}

const AuthApi = {
	logIn: async (login: string, password: string) => {
		return ApiJsonRequest(`${AUTH_URL}/log-in`, {
			method: "POST",
			body: JSON.stringify({ login, password }),
		})
	},

	logOut: async () => {
		return ApiJsonRequest(`${AUTH_URL}/log-out`, {
			method: "POST",
		})
	},

	registration: async (data: TUsersCreateDto) => {
		return ApiJsonRequest<TUserResponse>(`${AUTH_URL}/registration`, {
			method: "POST",
			body: JSON.stringify(data),
		})
	},

	downloadRegistrationCode: async () => {
		const result = await ApiJsonRequest<{ registrationCode: string }>(
			`${AUTH_URL}/registration-code`,
			{
				method: "POST",
			},
		)
		return result.registrationCode
	},

	refresh: async () => {
		return ApiJsonRequest(`${AUTH_URL}/refresh`, {
			method: "POST",
		})
	},
}
export default AuthApi
