import { GroupApi } from "@/shared/api/group/GroupApi"
import { AuthClient } from "../../_page/auth/auth-client"
import { createApiErrorHandler } from "@/shared/ApiError/createApiErrorHandler"
import { ApiErrors } from "@/shared/ApiError/ApiError"
import { DepartmentApi } from "@/shared/api/department"
import { error } from "console"
import { RolesApi } from "@/shared/api/roles"

const getGroups = async () => {
	const handleGetGroupsError = createApiErrorHandler([
		{
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

const getDepartments = async () => {
	const handleGetDepartmentsError = createApiErrorHandler([
		{
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

const getRoles = async () => {
	const handleGetRolesError = createApiErrorHandler([
		{
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

const AuthPage = async () => {
	const groups = await getGroups()
	const departments = await getDepartments()
	const roles = await getRoles()

	console.log(groups)
	return (
		<main className="content-center h-screen w-screen overflow-auto">
			<AuthClient
				groups={groups}
				departments={departments}
				roles={roles}
			/>
		</main>
	)
}

export default AuthPage
