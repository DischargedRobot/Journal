import { DepartmentCard } from "@/entities/department"
import { mockDepartments, mockProfessors } from "@/shared/model/mocks"

const Structure = () => {
	return (
		<div>
			<DepartmentCard
				department={mockDepartments[0]}
				professors={mockProfessors}
			/>
		</div>
	)
}

export default Structure
