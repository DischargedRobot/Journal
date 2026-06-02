import { StudentTable } from "@/entities/student"
import { TStudent } from "@/shared/model/student"

interface Props {
    students: TStudent[]
}

const PersonalStudentTable = ({ students }: Props) => {


    return (
        <StudentTable students={students} />
    )

}

export default PersonalStudentTable