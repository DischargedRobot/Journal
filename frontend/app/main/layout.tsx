import { ClientDataProvider } from "./ClientDataProvider"
import { TGroup } from "@/shared/model/group"
import { TDepartment } from "@/shared/model/department"
import { TUser } from "@/shared/model/user"

interface Props {
	children: React.ReactNode
	groups: TGroup[]
	departments: TDepartment[]
	user: TUser
}

const MainLayout = async (props: Props) => {
	const { children, groups, departments, user } = props
	return (
		<ClientDataProvider
			groups={groups}
			departments={departments}
			user={user}
		>
			{children}
		</ClientDataProvider>
	)
}

export default MainLayout
