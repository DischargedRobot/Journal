import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { MAIN_URL } from "@/shared/api/constants"
import { TPagedRequestOptions, TPagedResponse } from "@/shared/api/TPaged"
import { IBaseEntityWithVersion } from "@/shared/model/utility-types/base-entity"
import { Uuid } from "@/shared/model/utility-types/uuid"
import { buildQuery } from "../build-query"

export type TGroupResponseDto = {
	admissionDate: string
	code: string
	trainingDirectionUuid: Uuid
	facultyUuid: Uuid
	curatorsUuids: Uuid[]
} & IBaseEntityWithVersion

const GROUPS_URL = `${MAIN_URL}/groups`

export const GroupApi = {
	getGroupsWithoutEnhance: async (
		options?: TPagedRequestOptions,
	): Promise<TPagedResponse<TGroupResponseDto>> => {
		const query = buildQuery(options ?? {})
		const result = await ApiJsonRequest<TPagedResponse<TGroupResponseDto>>(
			`${GROUPS_URL}${query}`,
		)
		return result
	},
}
