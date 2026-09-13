import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"
import { TPagedRequestOptions, TPagedResponse } from "../TPaged"
import {
	TRole,
	TRoleRight,
	TRoleRightName,
	TRoleTypeName,
} from "@/shared/model/role"
import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { AUTH_URL } from "../constants"

const ROLES_URL = `${AUTH_URL}/roles`

type GetRolesOptions = TPagedRequestOptions & { roleTypeUuid: Uuid }

type role = {
	uuid: string
	name: string
	isBase: boolean
	rights: {
		uuid: string
		Name: string
	}[]
	roleTypes: {
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
		//TODO: обработать до TRole[]
		const items = result.items
		console.log("getroles")
		return items.map((item) => ({
			uuid: item.uuid,
			name: item.name,
			isBase: item.isBase,
			rights: item.rights
				.map((right) => ({
					uuid: right.uuid,
					name: right.Name,
					// Сужаем тип до TRoleRight
				}))
				.filter((right): right is TRoleRight =>
					TRoleRightName.includes(right.name as TRoleRightName),
				),
			roleTypes: item.roleTypes.map((roleType) => ({
				uuid: roleType.Uuid,
				name: roleType.Name as TRoleTypeName,
			})),
		}))
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
