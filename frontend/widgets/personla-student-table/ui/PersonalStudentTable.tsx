import { StudentTable } from "@/entities/student"
import { TStudent } from "@/shared/model/student"
import { AddField } from "@/shared/ui/add-field"
import { AddStudentForm } from "@/features/add-student-form"
import { Dispatch, SetStateAction, useState } from "react"

type ClassNames = {
	tableContainer?: string
	table?: string
	tableHeaderRow?: string
	tableBodyRow?: string
	tableHeaderCell?: string
	tableBodyCell?: string
}
interface Props {
	students: TStudent[]
}

const renderAddButton = (setIsOpen: Dispatch<SetStateAction<boolean>>) => (
	<AddField
		className="rounded-b-[20px] rounded-t-none w-full"
		label="Добавить студента"
		onClick={() => setIsOpen((prev) => !prev)}
	/>
)
const PersonalStudentTable = ({ students }: Props) => {
	const [isOpen, setIsOpen] = useState(false)
	return (
		<div className="flex flex-col flex-4 gap-4 mx-16 my-8 ">
			<StudentTable
				students={students}
				footer={renderAddButton(setIsOpen)}
			/>
			<AddStudentForm isOpen={isOpen} />
		</div>
	)
}

export default PersonalStudentTable
