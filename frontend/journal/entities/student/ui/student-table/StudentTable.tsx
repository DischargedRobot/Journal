import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import { TStudent } from '@/shared/model/student';
import { TBrigade } from '@/shared/model/brigade';
import { MoreToolsButton, type TMenuItemConfig } from '@/shared/ui/more-tools-button';
import { RoleGroup } from '@/shared/ui/role';

type BrigadeColor = { bg: string; text: string }

const BRIGADE_COLOR_POOL: BrigadeColor[] = [
    { bg: "#D4ED9A", text: "#000000" },
    { bg: "#B8D9F5", text: "#000000" },
    { bg: "#FFD700", text: "#000000" },
    { bg: "#FFA500", text: "#FFFFFF" },
    { bg: "#FF6B6B", text: "#FFFFFF" },
    { bg: "#800080", text: "#FFFFFF" },
]

const getBrigade = (student: TStudent): TBrigade | null =>
    student.brigades[0] ?? null

const buildBrigadeColorMap = (students: TStudent[]): Map<string, BrigadeColor> => {
    // Собираем все бригады из студентов и сортируем их по uuid
    const brigadeUuids = [
        ...new Set(
            students.flatMap((student) =>
                student.brigades.map((brigade) => brigade.uuid),
            ),
        ),
    ].sort()

    // Создаем мапу с цветами для каждой бригады
    return new Map(
        brigadeUuids.map((uuid, index) => [
            uuid,
            BRIGADE_COLOR_POOL[index % BRIGADE_COLOR_POOL.length],
        ]),
    )
}

const moreToolsButtonItems: TMenuItemConfig[] = [
    {
        key: "download",
        label: "Скачать список",
        onClick: () => { },

    },
    {
        key: "print",
        label: "Расчптать список",
        onClick: () => { },
    },
    {
        key: "create-brigade-template",
        label: "Создать шаблон бригады",
        onClick: () => { },
    },
    {
        key: "delete",
        label: "Удалить",
        onClick: () => { },
        sx: {
            color: "warning.main",
        },
    },


]

const StudentTable = ({ students }: { students: TStudent[] }) => {
    const brigadeColorByUuid = buildBrigadeColorMap(students)
    const hasBrigade = students.some((student) => getBrigade(student))

    return (
        <TableContainer className="rounded-[20px]" >
            <Table>
                <TableHead
                    sx={{
                        position: "sticky",
                        top: 0,
                        "& .MuiTableCell-head": {
                            backgroundColor: "primary.main",
                            color: "primary.contrastText",
                        },
                    }}
                >
                    <TableRow>
                        {hasBrigade && <TableCell width={100}>Бригада</TableCell>}
                        <TableCell>Фамилия И.О.</TableCell>
                        <TableCell width={150}>Студ. билет</TableCell>
                        <TableCell width={100}>Группа</TableCell>
                        <TableCell width={100}>Роли</TableCell>
                        <TableCell width={100}>
                            <MoreToolsButton items={moreToolsButtonItems} sx={{ color: "inherit" }} />
                        </TableCell>
                    </TableRow>
                </TableHead>
                <TableBody sx={{ backgroundColor: "secondary.light" }}>
                    {students.map((student) => {
                        const brigade = getBrigade(student)
                        const brigadeColor = brigade
                            ? brigadeColorByUuid.get(brigade.uuid)
                            : undefined

                        return (
                            <TableRow key={student.uuid}>
                                {hasBrigade && (brigade
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
                                        {brigade.name}
                                    </TableCell>
                                    : <TableCell />)}
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
