import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"
import { TPagedRequestOptions, TPagedResponse } from "../TPaged"
import { TRole } from "@/shared/model/role"
import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { AUTH_URL } from "../constants"

const GROUPS_URL = `${AUTH_URL}/groups`

type GetRolesOptions = TPagedRequestOptions & { roleTypeUUID: Uuid }

const RolesApi = {
	getRoles: async (options?: TPagedRequestOptions): Promise<TRole[]> => {
		const query = buildQuery(options ?? {})

		const result = await ApiJsonRequest<TPagedResponse<TRole>>(
			`${AUTH_URL}${query}`,
		)
		return result.items
	},
}

export default RolesApi
