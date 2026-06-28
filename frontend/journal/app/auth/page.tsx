"use client"

import { GroupApi } from "@/shared/api/group/GroupApi"
import { AuthClient } from "../../_page/auth/auth-client"
import { useEffect, useState } from "react"
import { TGroup } from "@/shared/model/group"

const AuthPage = () => {
	const [groups, setGroups] = useState<TGroup[]>([])

	useEffect(() => {
		const loadGroups = async () => {
			const groups = await GroupApi.getGroupsWithoutEnhance()
			setGroups(groups.items)
		}

		loadGroups()
	}, [])

	return (
		<main className="content-center h-screen w-screen overflow-auto">
			<AuthClient groups={groups} />
		</main>
	)
}

export default AuthPage
