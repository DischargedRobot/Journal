import { MAIN_URL } from "@/shared/api/constants"
import { ApiJsonRequest } from "@/shared/api/api-json-request"
import { TDepartment } from "@/shared/model/department"
import { TPagedRequestOptions } from "../TPaged/TPagedRequestOptions"
import { buildQuery } from "../build-query"
import { TWithoutEnrich } from "@/shared/lib/enricher/enrichTypes"
import { TPagedResponse } from "../TPaged"

const DEPARTMENTS_URL = `${MAIN_URL}/departments`

export type TDepartmentWE = TWithoutEnrich<TDepartment>

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
