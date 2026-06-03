import { useState } from "react"
import type { Dispatch, SetStateAction, ReactNode } from "react"
import { QRCode } from "@/shared/ui/qr-code"
import { CopyField } from "@/shared/ui/copy-field"
import Box from "@mui/material/Box"
import RadioGroup from "@mui/material/RadioGroup"
import FormControlLabel from "@mui/material/FormControlLabel"
import Radio from "@mui/material/Radio"
import RoleGroup from "@/shared/ui/role/RoleGroup"


interface Props {
    addButton: (setIsOpen: Dispatch<SetStateAction<boolean>>) => ReactNode
}

export const AddStudentForm = (props: Props) => {
    const { addButton } = props

    const [isOpen, setIsOpen] = useState(false)

    return (
        <Box className="flex flex-col gap-4">
            {addButton(setIsOpen)}
            {isOpen && (
                <Box
                    className="flex gap-4 p-4 rounded-[20px] w-fit"
                    sx={{
                        backgroundColor: "white",
                    }}
                >
                    <QRCode value="1234567890" />
                    <Box className="flex flex-col gap-2">
                        <CopyField value="1234567890" />
                        <RadioGroup
                            name="role"
                            defaultValue="Студент"
                            row
                        >
                            <FormControlLabel value="STUDENT" control={<Radio />} label="Студент" />
                            <FormControlLabel value="GROUP" control={<Radio />} label="Группа" />
                        </RadioGroup>
                        <RoleGroup
                            roles={[
                                { uuid: "1", name: "Студент", permissions: [] },
                            ]}
                            onAddRole={() => { }}
                            onClickRole={() => { }}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    )
}
export default AddStudentForm