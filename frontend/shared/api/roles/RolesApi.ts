import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"
import { TPagedRequestOptions, TPagedResponse } from "../TPaged"
import {
	TRole,
	TRoleRight,
	TRoleRightName,
	TRoleTypeName,
} from "@/shared/model/role"
import { ApiJsonRequest } from "@/shared/api/api-json-request"
import { AUTH_URL } from "../constants"
import { TWithoutEnrich } from "@/shared/lib/enricher"

const ROLES_URL = `${AUTH_URL}/roles`

type GetRolesOptions = TPagedRequestOptions & { roleTypeUuid: Uuid }

export type TRoleWE = TWithoutEnrich<TRole>

const RolesApi = {
	getRoles: async (options?: TPagedRequestOptions): Promise<TRole[]> => {
		const query = buildQuery(options ?? {})
		console.log(`getroles ${ROLES_URL}${query}`)

		const result = await ApiJsonRequest<TPagedResponse<TRole>>(
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
					name: right.name,
					// Сужаем тип до TRoleRight
				}))
				.filter((right): right is TRoleRight =>
					TRoleRightName.includes(right.name as TRoleRightName),
				),
			roleTypes: item.roleTypes.map((roleType) => ({
				uuid: roleType.uuid,
				name: roleType.name as TRoleTypeName,
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
