"use client"

import { TDepartment, useDepartmentStore } from "@/shared/model/department"
import { TGroup, useGroupStore } from "@/shared/model/group"
import { TUser, useUserStore } from "@/shared/model/user"
import { useLayoutEffect } from "react"

interface Props {
	children: React.ReactNode
	groups: TGroup[]
	departments: TDepartment[]
	user: TUser
}

export const ClientDataProvider = (props: Props) => {
	const { children, user, departments, groups } = props

	const storeUser = useUserStore((state) => state.user)
	const setUser = useUserStore((state) => state.setUser)
	const setDepartments = useDepartmentStore((state) => state.setDepartments)
	const setGroups = useGroupStore((state) => state.setGroups)

	useLayoutEffect(() => {
		setUser(user)
		setDepartments(departments)
		setGroups(groups)
	}, [])

	// чтобы children не работали до установки пользователя
	if (storeUser.uuid !== user.uuid) {
		return null
	}

	return <>{children}</>
}
