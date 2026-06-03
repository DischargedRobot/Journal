import { StudentTable } from "@/entities/student"
import { TStudent } from "@/shared/model/student"

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
    classNames?: ClassNames
}

const PersonalStudentTable = ({ students, classNames }: Props) => {

    return (
        <StudentTable students={students} classNames={classNames} />
    )

}

export default PersonalStudentTable