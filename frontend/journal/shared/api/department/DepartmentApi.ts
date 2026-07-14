import { MAIN_URL } from "@/shared/api/constants"

const DEPARTMENTS_URL = `${MAIN_URL}/departments`

export const DepartmentApi = {
	getDepartments: async () => {
		const response = await fetch(`${DEPARTMENTS_URL}`)
		return response.json()
	},
}
