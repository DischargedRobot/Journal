"use client"

import MuiCheckbox, { CheckboxProps as MuiCheckboxProps } from "@mui/material/Checkbox"
import CheckboxCheckedIcon from "./CheckboxCheckedIcon"
import CheckboxUncheckedIcon from "./CheckboxUncheckedIcon"

export type CheckboxProps = MuiCheckboxProps

const Checkbox = ({ sx, ...props }: CheckboxProps) => (
	<MuiCheckbox
		{...props}
		icon={<CheckboxUncheckedIcon />}
		checkedIcon={<CheckboxCheckedIcon />}
		sx={[
			{
				padding: 0,
			},
			...(Array.isArray(sx) ? sx : [sx]),
		]}
	/>
)

export default Checkbox
