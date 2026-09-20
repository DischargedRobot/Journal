import { ApiJsonRequest } from "@/shared/api/api-json-request"
import { MAIN_URL } from "@/shared/api/constants"
import { TPagedRequestOptions, TPagedResponse } from "@/shared/api/TPaged"
import { IBaseEntityWithVersion } from "@/shared/model/utility-types/base-entity"
import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"
import { TGroup } from "@/shared/model/group"
import { TWithoutEnrich } from "@/shared/lib/enricher/enrichTypes"

export type TGroupResponseDto = {
	admissionDate: string
	code: string
	trainingDirectionUuid: Uuid
	facultyUuid: Uuid
	curatorsUuids: Uuid[]
} & IBaseEntityWithVersion

const GROUPS_URL = `${MAIN_URL}/groups`

export type TGroupWE = TWithoutEnrich<TGroup>

export const GroupApi = {
	getGroupsWithoutEnrich: async (
		options?: TPagedRequestOptions,
	): Promise<TPagedResponse<TGroupWE>> => {
		const query = buildQuery(options ?? {})
		const result = await ApiJsonRequest<TPagedResponse<TGroupWE>>(
			`${GROUPS_URL}${query}`,
		)
		return result
	},
}
