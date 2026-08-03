import { MAIN_URL } from "@/shared/api/constants"
import ApiJsonRequest from "@/shared/ApiError/ApiJsonRequest"
import { TDepartment } from "@/shared/model/t-department"
import { TPagedRequestOptions } from "../TPaged/TPagedRequestOptions"
import { buildQuery } from "../build-query"
import { Uuid } from "@/shared/model/utility-types/uuid"
import { TPagedResponse } from "../TPaged/TPagedResponse"
import { IBaseEntityWithVersion } from "@/shared/model/utility-types/base-entity"

const DEPARTMENTS_URL = `${MAIN_URL}/departments`

export type TDepartmentResponseDto = {
	name: string
	shortName: string
	facultyUuid: Uuid
	code: string
} & IBaseEntityWithVersion

export const DepartmentApi = {
	getDepartmentsWithoutEnhance: async (options?: TPagedRequestOptions): Promise<TDepartmentResponseDto[]> => {
		const query = buildQuery(options ?? {})
		const response = await ApiJsonRequest<TPagedResponse<TDepartmentResponseDto>>(`${DEPARTMENTS_URL}${query}`)

		return response.items
	},
}
