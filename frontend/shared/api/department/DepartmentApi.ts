import { MAIN_URL } from "@/shared/api/constants"
import { ApiJsonRequest } from "@/shared/api/api-json-request"
import { TDepartment } from "@/shared/model/department"
import { TPagedRequestOptions } from "../TPaged/TPagedRequestOptions"
import { buildQuery } from "../build-query"
import { Uuid } from "@/shared/model/utility-types/uuid"
import { TPagedResponse } from "../TPaged/TPagedResponse"
import { IBaseEntityWithVersion } from "@/shared/model/utility-types/base-entity"

const DEPARTMENTS_URL = `${MAIN_URL}/departments`

export type TDepartmentWE = {
	name: string
	shortName: string
	facultyUuid: Uuid
	code: string
} & IBaseEntityWithVersion

export const DepartmentApi = {
	getDepartmentsWithoutEnrich: async (
		options?: TPagedRequestOptions,
	): Promise<TDepartmentWE[]> => {
		const query = buildQuery(options ?? {})
		const response = await ApiJsonRequest<TPagedResponse<TDepartmentWE>>(
			`${DEPARTMENTS_URL}${query}`,
		)

		return response.items
	},
}
