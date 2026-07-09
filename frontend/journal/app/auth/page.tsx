"use client"

import { GroupApi } from "@/shared/api/group/GroupApi"
import { AuthClient } from "../../_page/auth/auth-client"
import { useEffect, useState } from "react"
import { TGroup } from "@/shared/model/group"
import { createApiErrorHandler } from "@/shared/ApiError/createApiErrorHandler"
import { ApiErrors } from "@/shared/ApiError/ApiError"

const AuthPage = () => {
	const [groups, setGroups] = useState<TGroup[]>([])

	const handleGetGroupsError = createApiErrorHandler([
		{
			error: ApiErrors.BAD_REQUEST,
			handler: (error) => {
				console.log(error)
			},
		},
	])

	useEffect(() => {
		const loadGroups = async () => {
			try {
				const groups = await GroupApi.getGroupsWithoutEnhance()
				setGroups(groups.items)
			} catch (error) {
				handleGetGroupsError(error)
			}
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
