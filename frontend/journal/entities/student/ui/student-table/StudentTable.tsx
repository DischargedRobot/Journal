import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import { TStudent } from '@/shared/model/student';
import { MoreToolsButton } from '@/shared/ui/more-tools-button';
import { RoleGroup } from '@/shared/ui/role';

const BRIGADE_COLORS: Record<number, { bg: string; text: string }> = {
    1: { bg: "#D4ED9A", text: "#000000" },
    2: { bg: "#FFD700", text: "#000000" },
    3: { bg: "#FFA500", text: "#FFFFFF" },
    4: { bg: "#FF0000", text: "#FFFFFF" },
    5: { bg: "#800080", text: "#FFFFFF" },
    6: { bg: "#008000", text: "#FFFFFF" },
}

const getBrigadeNumber = (student: TStudent): number | null => {
    const brigade = student.brigades[0]
    if (!brigade) {
        return null
    }

    const parsed = Number.parseInt(brigade.name, 10)
    return Number.isFinite(parsed) ? parsed : null
}

const getBrigadeColor = (brigadeNumber: number | null) => {
    if (!brigadeNumber || brigadeNumber < 1) {
        return null
    }

    return BRIGADE_COLORS[brigadeNumber]
}

const StudentTable = ({ students }: { students: TStudent[] }) => {
    return (
        <TableContainer sx={{ overflow: "visible" }}>
            <Table sx={{ overflow: "visible" }}>
                <TableHead >
                    <TableRow >
                        <TableCell width={100}>Бригада</TableCell>
                        <TableCell>Фамилия И.О.</TableCell>
                        <TableCell width={150}>Студ. билет</TableCell>
                        <TableCell width={100}>Группа</TableCell>
                        <TableCell width={100}>Роли</TableCell>
                        <TableCell width={100}><MoreToolsButton items={[]} /></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {students.map((student) => {
                        const brigadeNumber = getBrigadeNumber(student)
                        const brigadeColor = getBrigadeColor(brigadeNumber)

                        return (
                            <TableRow key={student.uuid}>
                                {brigadeNumber
                                    ? <TableCell
                                        className="flex items-center justify-center rounded-full w-10 h-10"
                                        sx={
                                            brigadeColor
                                                ? {
                                                    backgroundColor: brigadeColor.bg,
                                                    color: brigadeColor.text,
                                                }
                                                : undefined
                                        }
                                    >
                                        {brigadeNumber}
                                    </TableCell>
                                    : <TableCell />}
                                <TableCell>
                                    {student.lastName} {student.firstName} {student.patronymic}
                                </TableCell>
                                <TableCell>{student.group.code}</TableCell>
                                <TableCell>{student.studentCode}</TableCell>
                                <TableCell><RoleGroup roles={student.roles} /></TableCell>
                                <TableCell><MoreToolsButton items={[]} /></TableCell>
                            </TableRow>
                        )
                    })}
                </TableBody>
            </Table>
        </TableContainer>
    )
}

export default StudentTable
