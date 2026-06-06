"use client"

import { DepartmentCard } from "@/entities/department"
import { mockDepartments, mockProfessors } from "@/shared/model/mocks"
import { AddDepartment } from "@/widgets/add-department"

const Structure = () => {
	return (
		<div>
			<DepartmentCard
				department={mockDepartments[0]}
				professors={mockProfessors}
			/>
			<AddDepartment
				onClick={() => {
					console.log("add department")
				}}
			/>
		</div>
	)
}

export default Structure
