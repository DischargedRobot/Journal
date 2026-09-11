import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { AUTH_URL } from "../constants"
import { buildQuery } from "../build-query"
import { TPagedRequestOptions, TPagedResponse } from "../TPaged"
import { TRole, TRoleRight } from "@/shared/model/role"

const ROLE_RIGHT_URL = `${AUTH_URL}/RoleRights`

export const RoleRightApi = {
	getRoleRights: async (
		options?: TPagedRequestOptions,
	): Promise<TRoleRight[]> => {
		const query = buildQuery(options ?? {})
		const result = await ApiJsonRequest<TPagedResponse<TRoleRight>>(
			`${ROLE_RIGHT_URL}${query}`,
		)

		return result.items
	},
}
