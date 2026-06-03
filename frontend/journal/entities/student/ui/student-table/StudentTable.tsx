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

type ClassNames = {
    tableContainer?: string
    table?: string
    tableHead?: string
    tableBody?: string
    tableHeaderRow?: string
    tableBodyRow?: string
    tableHeaderCell?: string
    tableBodyCell?: string
}

const flexCell = { display: "block", boxSizing: "border-box" } as const
const columnSx = {
    brigade: { ...flexCell, width: 100, flexShrink: 0 },
    name: { ...flexCell, flex: 1, minWidth: 160 },
    studentCode: { ...flexCell, width: 180, flexShrink: 0 },
    group: { ...flexCell, width: 80, flexShrink: 0 },
    roles: { ...flexCell, width: 280, flexShrink: 0 },
    actions: { ...flexCell, width: 50, flexShrink: 0 },
} as const

interface Props {
    students: TStudent[]
    classNames?: ClassNames
}

const StudentTable = ({ students, classNames }: Props) => {
    const brigadeColorByUuid = buildBrigadeColorMap(students)
    const hasBrigade = students.some((student) => getBrigade(student))

    return (
        <TableContainer className={`rounded-[20px] h-fit ${classNames?.tableContainer}`} >
            <Table className={classNames?.table}>
                <TableHead
                    className={classNames?.tableHead}
                    sx={{
                        position: "sticky",
                        top: 0,
                        backgroundColor: "primary.main",

                        "& .MuiTableCell-head": {
                            color: "primary.contrastText",
                            textAlign: "center",
                            fontWeight: "normal",
                        },
                    }}

                >
                    <TableRow className={`flex gap-[15px] px-1.25 ${classNames?.tableHeaderRow}`}>
                        {hasBrigade && <TableCell sx={columnSx.brigade} className={`py-2.5 text-start text content-end ${classNames?.tableHeaderCell}`}>Бригада</TableCell>}
                        <TableCell sx={columnSx.name} className={`py-2.5 text-center text content-end ${classNames?.tableHeaderCell}`}>Фамилия И.О.</TableCell>
                        <TableCell sx={columnSx.studentCode} className={`py-2.5 text-center text content-end ${classNames?.tableHeaderCell}`}>Студ. билет</TableCell>
                        <TableCell sx={columnSx.group} className={`py-2.5 text-center text content-end ${classNames?.tableHeaderCell}`}>Группа</TableCell>
                        <TableCell sx={columnSx.roles} className={`py-2.5 text-center text content-end ${classNames?.tableHeaderCell}`}>Роли</TableCell>
                        <TableCell sx={columnSx.actions} className={`py-2.5 text-center text content-end ${classNames?.tableHeaderCell}`}>
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
                            <TableRow key={student.uuid}
                                className={`flex gap-[15px] px-1.25 ${classNames?.tableBodyRow}`}>
                                {hasBrigade && (brigade
                                    ? <TableCell
                                        className={`flex items-center justify-center rounded-full h-10 text-center text ${classNames?.tableBodyCell}`}
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
                                <TableCell sx={columnSx.name} className={`text-start text ${classNames?.tableBodyCell}`}>
                                    {student.lastName} {student.firstName} {student.patronymic}
                                </TableCell>
                                <TableCell sx={columnSx.studentCode} className={`text-center text ${classNames?.tableBodyCell}`}>{student.studentCode}</TableCell>
                                <TableCell sx={columnSx.group} className={`text-center text ${classNames?.tableBodyCell}`}>{student.group.code}</TableCell>
                                <TableCell sx={columnSx.roles} className={`text-center text ${classNames?.tableBodyCell}`}><RoleGroup roles={student.roles} /></TableCell>
                                <TableCell sx={columnSx.actions} className={`text-center text ${classNames?.tableBodyCell}`}>
                                    <MoreToolsButton items={[

                                        {
                                            key: "select",
                                            label: "Выбрать",
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
                                    ]} />
                                </TableCell>
                            </TableRow>
                        )
                    })}
                </TableBody>
            </Table>
        </TableContainer >
    )
}

export default StudentTable
