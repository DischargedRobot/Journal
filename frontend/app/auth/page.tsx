import {
	getDepartments,
	getGroups,
	getRoles,
} from "@/_page/auth/api/getAuthPageData"
import { AuthClient } from "@/_page/auth/ui/auth-client"

const AuthPage = async () => {
	const groups = await getGroups()
	const departments = await getDepartments()
	const roles = await getRoles()

	// const roleRights = await getRoleRight()

	// console.log(roles, "roles auth page")
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
