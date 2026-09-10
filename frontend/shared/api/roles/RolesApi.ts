import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"
import { TPagedRequestOptions, TPagedResponse } from "../TPaged"
import { TRole } from "@/shared/model/role"
import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { AUTH_URL } from "../constants"

const ROLES_URL = `${AUTH_URL}/roles`

type GetRolesOptions = TPagedRequestOptions & { roleTypeUuid: Uuid }

type role = {
	Uuid: string
	Name: string
	IsBase: boolean
	Rights: {
		Uuid: string
		Name: string
	}[]
	RoleTypes: {
		Uuid: string
		Name: string
	}[]
}

const RolesApi = {
	getRoles: async (options?: TPagedRequestOptions): Promise<TRole[]> => {
		const query = buildQuery(options ?? {})
		console.log(`getroles ${ROLES_URL}${query}`)

		const result = await ApiJsonRequest<TPagedResponse<role>>(
			`${ROLES_URL}${query}`,
		)
		console.log("getroles ss", result)

		const iemtes = result.items
		console.log("getroles")
		return []
	},

	getRolesByRoleTypeUuid: async (
		options?: GetRolesOptions,
	): Promise<TRole[]> => {
		const query = buildQuery(options ?? {})

		const result = await ApiJsonRequest<TPagedResponse<TRole>>(
			`${AUTH_URL}${query}`,
		)
		return result.items
	},
}

export default RolesApi
