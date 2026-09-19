import { ApiErrors, createApiErrorHandler } from "@/shared/api/api-error"
import { DepartmentApi } from "@/shared/api/department"
import { GroupApi } from "@/shared/api/group"
import { RolesApi } from "@/shared/api/roles"

export const getGroups = async () => {
	const handleGetGroupsError = createApiErrorHandler([
		{
			// TODO: добавить обработку ошибки 404
			error: ApiErrors.BAD_REQUEST,
			handler: (error) => {
				console.log(error)
			},
		},
	])

	try {
		const response = await GroupApi.getGroupsWithoutEnhance()
		return response.items
	} catch (error) {
		handleGetGroupsError(error)
		return []
	}
}

export const getDepartments = async () => {
	const handleGetDepartmentsError = createApiErrorHandler([
		{
			// TODO: добавить обработку ошибки 404
			error: ApiErrors.BAD_REQUEST,
			handler: (error) => {
				console.log(error)
			},
		},
	])

	try {
		return await DepartmentApi.getDepartmentsWithoutEnhance()
	} catch (error) {
		handleGetDepartmentsError(error)
		return []
	}
}

export const getRoles = async () => {
	const handleGetRolesError = createApiErrorHandler([
		{
			// TODO: добавить обработку ошибки 404
			error: ApiErrors.NOT_FOUND,
			handler: (error) => {
				console.log(error)
			},
		},
	])

	try {
		return await RolesApi.getRoles()
	} catch (error) {
		handleGetRolesError(error)
		return []
	}
}
